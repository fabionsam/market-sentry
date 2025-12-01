import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatDialogRef, MatDialogModule } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { StockService } from '../../services/stock.service';

@Component({
  selector: 'app-stock-dialog',
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatDialogModule,
    MatButtonModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule
  ],
  templateUrl: './stock-dialog.html',
  styleUrl: './stock-dialog.scss',
})
export class StockDialog {
  private fb = inject(FormBuilder);
  private dialogRef = inject(MatDialogRef<StockDialog>);
  private stockService = inject(StockService);

  // Signal para guardar a lista de APIs que virá do banco
  apiOptions = signal<any[]>([]);

  form = this.fb.group({
    symbol: ['', Validators.required],
    priceBuy: [0, [Validators.required, Validators.min(0.01)]],
    priceSell: [0, [Validators.required, Validators.min(0.01)]],
    emailNotification: ['', [Validators.required, Validators.email]],
    apiId: [1, Validators.required],
    isActive: [true]
  });

  ngOnInit() {
    // Carrega as APIs assim que o modal abre
    this.stockService.getApis().subscribe({
      next: (data) => {
        this.apiOptions.set(data);
        if (data.length > 0) {
           this.form.patchValue({ apiId: data[0].id });
        }
      },
      error: (err) => {}
    });
  }

  save() {
    if (this.form.valid) {
      this.dialogRef.close(this.form.value);
    }
  }

  cancel() {
    this.dialogRef.close(null);
  }
}