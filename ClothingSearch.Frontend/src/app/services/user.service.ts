import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { ApiService, UserSettings, UpdateUserSettings, Country } from './api.service';

@Injectable({
  providedIn: 'root'
})
export class UserService {
  private currentUserSubject = new BehaviorSubject<UserSettings | null>(null);
  public currentUser$ = this.currentUserSubject.asObservable();
  
  private userIdKey = 'clothingsearch_user_id';

  constructor(private apiService: ApiService) {
    this.initializeUser();
  }

  private initializeUser() {
    let userId = localStorage.getItem(this.userIdKey);
    if (!userId) {
      userId = this.generateUserId();
      localStorage.setItem(this.userIdKey, userId);
    }

    this.loadUserSettings(userId).subscribe({
      next: (settings) => this.currentUserSubject.next(settings),
      error: () => this.createDefaultSettings(userId)
    });
  }

  private generateUserId(): string {
    return 'user_' + Date.now() + '_' + Math.random().toString(36).substr(2, 9);
  }

  getCurrentUserId(): string {
    return localStorage.getItem(this.userIdKey) || 'anonymous';
  }

  getCurrentUser(): UserSettings | null {
    return this.currentUserSubject.value;
  }

  private loadUserSettings(userId: string): Observable<UserSettings> {
    return this.apiService.getUserSettings(userId);
  }

  private createDefaultSettings(userId: string) {
    const defaultSettings: UpdateUserSettings = {
      countryId: 1, // Croatia
      clothingSize: 'M',
      shoeSize: '42',
      shoeSizeSystem: 'EU'
    };

    this.apiService.updateUserSettings(userId, defaultSettings).subscribe({
      next: (settings) => this.currentUserSubject.next(settings),
      error: (error) => console.error('Failed to create default settings:', error)
    });
  }

  updateSettings(settings: UpdateUserSettings): Observable<UserSettings> {
    const userId = this.getCurrentUserId();
    return this.apiService.updateUserSettings(userId, settings);
  }

  refreshUserSettings() {
    const userId = this.getCurrentUserId();
    this.loadUserSettings(userId).subscribe({
      next: (settings) => this.currentUserSubject.next(settings),
      error: (error) => console.error('Failed to refresh user settings:', error)
    });
  }
}
