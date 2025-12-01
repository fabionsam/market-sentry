import { HttpInterceptorFn, HttpErrorResponse } from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError, throwError } from 'rxjs';
import { NotificationService } from '../services/notification';

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const notificationService = inject(NotificationService);

  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {
      let errorMessage = 'Ocorreu um erro inesperado.';

      if (error.error instanceof ErrorEvent) {
        errorMessage = `Erro: ${error.error.message}`;
      } else {
        // Erro do lado do servidor (ex: 404, 500)
        // Se a API mandar uma mensagem específica, usamos ela
        if (error.status === 0) {
           errorMessage = 'Não foi possível conectar ao servidor (API offline).';
        } else if (error.error && typeof error.error === 'string') {
           errorMessage = error.error; 
        } else {
           errorMessage = `Erro ${error.status}: ${error.statusText}`;
        }
      }

      // Mostra o Toast na direita em cima
      notificationService.showError(errorMessage);

      // Relança o erro para o componente saber que falhou (e parar loadings)
      return throwError(() => error);
    })
  );
};