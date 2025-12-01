import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatTabsModule } from '@angular/material/tabs';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatCardModule } from '@angular/material/card';
import { MatSnackBar } from '@angular/material/snack-bar'; 
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { NotificationService } from '../../services/notification';

@Component({
  selector: 'app-settings',
  imports: [
    CommonModule, ReactiveFormsModule, MatTabsModule, 
    MatFormFieldModule, MatInputModule, MatButtonModule, 
    MatIconModule, MatCardModule
  ],
  templateUrl: './settings.html',
  styleUrl: './settings.scss',
})
export class Settings implements OnInit {
  private fb = inject(FormBuilder);
  private http = inject(HttpClient);
  private snackBar = inject(MatSnackBar);
  private router = inject(Router);
  private notification = inject(NotificationService);

  // URL Base da API
  private apiUrl = '/api'; 

  // --- SMTP FORM ---
  smtpForm = this.fb.group({
    host: ['', Validators.required],
    port: [587, Validators.required],
    userName: [''],
    password: [''],
    senderEmail: ['no-reply@marketsentry.com', [Validators.required, Validators.email]],
    senderName: ['Market Sentry'],
    enableSsl: [true]
  });

  // --- API LIST (Signals) ---
  apiList = signal<any[]>([]);

  ngOnInit() {
    this.loadSmtp();
    this.loadApis();
  }

  // --- MÉTODOS SMTP ---
  loadSmtp() {
    this.http.get<any>(`${this.apiUrl}/Settings/smtp`).subscribe(data => {
      if (data && data.host) { 
        this.smtpForm.patchValue(data);
      }
    });
  }

  saveSmtp() {
    if (this.smtpForm.valid) {
      this.http.post(`${this.apiUrl}/Settings/smtp`, this.smtpForm.value).subscribe({
        next: () => this.notification.showSuccess('Configurações salvas com sucesso!'), 
        error: () => {}
      });
    }
  }

  // --- MÉTODOS API ---
  loadApis() {
    this.http.get<any[]>(`${this.apiUrl}/ApiConfigurations`).subscribe(data => {
      this.apiList.set(data);
    });
  }

  // Atalho para voltar
  goBack() {
    this.router.navigate(['/']);
  }

  showMsg(msg: string) {
    this.snackBar.open(msg, 'Fechar', { duration: 3000 });
  }
}