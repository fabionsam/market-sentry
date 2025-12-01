import { ChangeDetectorRef, Component, inject, OnDestroy, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatExpansionModule } from '@angular/material/expansion'; 
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { BaseChartDirective } from 'ng2-charts';
import { ChartConfiguration, ChartOptions } from 'chart.js';
import { StockService } from '../../services/stock.service';
import { StockDialog } from '../../components/stock-dialog/stock-dialog';
import { MatDialog } from '@angular/material/dialog';
import { NotificationService } from '../../services/notification';
import { ConfirmDialog } from '../../components/confirm-dialog/confirm-dialog';
import { Subscription, interval } from 'rxjs';
import { startWith, switchMap } from 'rxjs/operators';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [
    CommonModule, MatExpansionModule, MatButtonModule, MatIconModule, BaseChartDirective
  ],
  templateUrl: './dashboard.html',
  styleUrl: './dashboard.scss'
})
export class Dashboard implements OnInit, OnDestroy {
  private stockService = inject(StockService);
  private dialog = inject(MatDialog);
  private cdr = inject(ChangeDetectorRef);
  private notification = inject(NotificationService);
  
  private activeSubscriptions: { [symbol: string]: Subscription } = {};
  private globalSubscription?: Subscription; 

  configs = signal<any[]>([]);
  loading = signal<boolean>(true);

  // Armazena dados dos gráficos por Símbolo
  chartDataMap: { [symbol: string]: ChartConfiguration<'line'>['data'] } = {};
  chartOptionsMap: { [symbol: string]: ChartOptions<'line'> } = {};
  


  baseChartOptions: ChartOptions<'line'> = {
    responsive: true,
    maintainAspectRatio: false,
    elements: { point: { radius: 2 } },
    plugins: { legend: { position: 'bottom' } },
    interaction: { mode: 'index', intersect: false },
    scales: {
      x: { display: true }, // Garante que o eixo X apareça
      y: { display: true }  // O eixo Y será sobrescrito dinamicamente
    }
  };

  ngOnInit() { 
    this.loadData(); 
    this.startGlobalPolling();
  }

  ngOnDestroy() {
    Object.values(this.activeSubscriptions).forEach(sub => sub.unsubscribe());
    if (this.globalSubscription) this.globalSubscription.unsubscribe();
  }

  loadData() {
    this.loading.set(true);
    this.stockService.getConfigs().subscribe({
      next: (data) => {
        this.configs.set(data);
        this.loading.set(false);
      },
      error: () => this.loading.set(false)
    });
  }

  startGlobalPolling() {
    // A cada 30 segundos
    this.globalSubscription = interval(30000).subscribe(() => {
      
      // Chama a API para pegar a lista atualizada
      this.stockService.getConfigs().subscribe({
        next: (newData) => {
          this.configs.set(newData);
        },
        error: (err) => console.error('Erro no update global', err)
      });
    });
  }

  onExpand(stock: any) {
    // Se já existe um timer rodando pra esse cara, não faz nada
    if (this.activeSubscriptions[stock.symbol]) return;

    // Cria o Polling (Requisição recorrente)
    this.activeSubscriptions[stock.symbol] = interval(30000) // A cada 30s (30000ms)
      .pipe(
        startWith(0), // Executa imediatamente (não espera os primeiros 30s)
        switchMap(() => this.stockService.getHistory(stock.symbol)) // Troca o timer pela chamada da API
      )
      .subscribe({
        next: (history) => {
          this.updateChart(stock, history);
        },
        error: (err) => console.error(`Erro ao atualizar ${stock.symbol}`, err)
      });
  }

  // --- LÓGICA DE FECHAMENTO (Mata o Timer) ---
  onClose(stock: any) {
    const sub = this.activeSubscriptions[stock.symbol];
    if (sub) {
      sub.unsubscribe(); // Para de pedir dados
      delete this.activeSubscriptions[stock.symbol]; // Remove da lista
    }
  }

  private updateChart(stock: any, history: any[]) {
    const labels = history.map(h => new Date(h.timestamp).toLocaleTimeString([], {hour:'2-digit', minute:'2-digit'}));
    const dataPrices = history.map(h => h.price);
    
    // Calcula linhas de alvo
    const buyLine = new Array(labels.length).fill(stock.priceBuy);
    const sellLine = new Array(labels.length).fill(stock.priceSell);

    // Calcula Min/Max para escala dinâmica
    const allValues = [...dataPrices, stock.priceBuy, stock.priceSell];
    const minVal = Math.min(...allValues) - 5; 
    const maxVal = Math.max(...allValues) + 5;

    // Atualiza Opções
    this.chartOptionsMap[stock.symbol] = {
      ...this.baseChartOptions,
      scales: {
        y: { min: minVal, max: maxVal }
      },
      animation: false 
    };

    // Atualiza Dados
    const chartData = {
      labels: labels,
      datasets: [
        { data: dataPrices, label: 'Preço Real', borderColor: '#007bff', backgroundColor: 'rgba(0,123,255,0.1)', fill: true, tension: 0.3 },
        { data: buyLine, label: 'Compra', borderColor: 'green', borderDash: [5,5], pointRadius: 0, borderWidth: 1 },
        { data: sellLine, label: 'Venda', borderColor: 'red', borderDash: [5,5], pointRadius: 0, borderWidth: 1 }
      ]
    };

    // Atualiza a tela
    setTimeout(() => {
      this.chartDataMap[stock.symbol] = chartData;
      this.cdr.detectChanges();
    }, 0);
  }

  openAddModal() {
    const dialogRef = this.dialog.open(StockDialog, { width: '600px' });
    dialogRef.afterClosed().subscribe(result => {
      if (result) this.createStock(result);
    });
  }

  createStock(newConfig: any) {
    this.stockService.createConfig(newConfig).subscribe(() => this.loadData());
  }

  deleteStock(event: Event, stock: any) {
    // 1. Impede que o accordion abra/feche ao clicar na lixeira
    event.stopPropagation(); 

   const dialogRef = this.dialog.open(ConfirmDialog, {
      width: '400px',
      data: {
        title: 'Excluir Monitoramento',
        message: `Tem certeza que deseja remover o ativo ${stock.symbol} da lista?`
      }
    });

    // ESPERA A RESPOSTA
    dialogRef.afterClosed().subscribe(result => {
      if (result === true) {
        // Se o usuário clicou em "Confirmar", executa a exclusão
        this.executeDelete(stock);
      }
    });
  }

  private executeDelete(stock: any) {
    this.loading.set(true);
    this.stockService.deleteConfig(stock.id).subscribe({
      next: () => {
        this.notification.showSuccess(`Ativo ${stock.symbol} removido.`);
        this.configs.update(list => list.filter(c => c.id !== stock.id));
        this.loading.set(false);
      },
      error: () => this.loading.set(false)
    });
  }
}