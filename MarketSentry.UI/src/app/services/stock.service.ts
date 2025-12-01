import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { StockConfig, ApiConfiguration } from '../models/stock-config.interface';

@Injectable({
  providedIn: 'root'
})
export class StockService {
  private apiUrl = '/api'; 

  constructor(private http: HttpClient) { }

  getConfigs(): Observable<StockConfig[]> {
    return this.http.get<StockConfig[]>(`${this.apiUrl}/StockConfigs`);
  }

  getApis(): Observable<ApiConfiguration[]> {
    return this.http.get<ApiConfiguration[]>(`${this.apiUrl}/ApiConfigurations`);
  }

  createConfig(config: StockConfig): Observable<StockConfig> {
    return this.http.post<StockConfig>(`${this.apiUrl}/StockConfigs`, config);
  }

  deleteConfig(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/StockConfigs/${id}`);
  }

  getHistory(symbol: string): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/StockConfigs/history/${symbol}`);
  }
}