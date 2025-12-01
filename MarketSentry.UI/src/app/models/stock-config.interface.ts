export interface ApiConfiguration {
  id: number;
  providerName: string;
  baseUrl: string;
  isActive: boolean;
}

export interface StockConfig {
  id: number;
  symbol: string;
  priceSell: number;
  priceBuy: number;
  emailNotification: string;
  isActive: boolean;
  apiId: number;
  api?: ApiConfiguration; 
}