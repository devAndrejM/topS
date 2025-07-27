// src/app/services/api.service.ts
import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

export interface Product {
  name: string;
  brand: string;
  price: number;
  currency: string;
  imageUrl: string;
  productUrl: string;
  affiliateUrl: string;
  storeName: string;
  category: string;
  sizes: string[];
  inStock: boolean;
  description: string;
}

export interface CategorySummary {
  name: string;
  count: number;
}

export interface SearchResult {
  query: string;
  categories: CategorySummary[];
  products: Product[];
  isFromCache: boolean;
  lastUpdated: string;
  totalResults: number;
  selectedCategory: string;
}

export interface Country {
  id: number;
  name: string;
  currency: string;
}

export interface UserSettings {
  userId: string;
  countryId: number;
  countryName: string;
  clothingSize: string;
  shoeSize: string;
  shoeSizeSystem: string;
}

export interface UpdateUserSettings {
  countryId: number;
  clothingSize: string;
  shoeSize: string;
  shoeSizeSystem: string;
}

@Injectable({
  providedIn: 'root'
})
export class ApiService {
  private baseUrl = environment.apiUrl;

  constructor(private http: HttpClient) { }

  search(query: string, userId: string = 'anonymous', category: string = ''): Observable<SearchResult> {
    let params = new HttpParams()
      .set('query', query)
      .set('userId', userId);
        
    if (category) {
      params = params.set('category', category);
    }

    // FIXED: Use backticks for template literals, not double quotes
    return this.http.get<SearchResult>(`${this.baseUrl}/api/search`, { params });
  }

  getCategories(query: string, userId: string = 'anonymous'): Observable<CategorySummary[]> {
    const params = new HttpParams()
      .set('query', query)
      .set('userId', userId);

    // FIXED: Use backticks for template literals
    return this.http.get<CategorySummary[]>(`${this.baseUrl}/api/search/categories`, { params });
  }

  getUserSettings(userId: string): Observable<UserSettings> {
    // FIXED: Use backticks and include userId in path
    return this.http.get<UserSettings>(`${this.baseUrl}/api/user/${userId}/settings`);
  }

  updateUserSettings(userId: string, settings: UpdateUserSettings): Observable<UserSettings> {
    // FIXED: Use backticks and include userId in path
    return this.http.post<UserSettings>(`${this.baseUrl}/api/user/${userId}/settings`, settings);
  }

  getCountries(): Observable<Country[]> {
    // FIXED: Use backticks for template literals
    return this.http.get<Country[]>(`${this.baseUrl}/api/user/countries`);
  }
}