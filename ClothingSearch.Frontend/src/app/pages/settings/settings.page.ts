import { Component, OnInit } from '@angular/core';
import { AlertController, ToastController } from '@ionic/angular';
import { Router } from '@angular/router';

export interface UserSettings {
  country: string;
  currency: string;
  clothingSize: string;
  shoeSize: string;
  sizeSystem: string;
  defaultSort: string;
  showUnavailable: boolean;
}

@Component({
  selector: 'app-settings',
  templateUrl: './settings.page.html',
  styleUrls: ['./settings.page.scss'],
})
export class SettingsPage implements OnInit {
  
  settings: UserSettings = {
    country: 'Croatia',
    currency: 'EUR',
    clothingSize: 'M',
    shoeSize: 'EU 42',
    sizeSystem: 'EU',
    defaultSort: 'price',
    showUnavailable: false
  };

  appVersion = '1.0.0';

  constructor(
    private alertController: AlertController,
    private toastController: ToastController,
    private router: Router
  ) {}

  ngOnInit() {
    this.loadSettings();
  }

  loadSettings() {
    const stored = localStorage.getItem('clothingSearchSettings');
    if (stored) {
      this.settings = { ...this.settings, ...JSON.parse(stored) };
    }
  }

  saveSettings() {
    // Update currency based on country
    this.settings.currency = this.getCurrencyForCountry(this.settings.country);
    
    localStorage.setItem('clothingSearchSettings', JSON.stringify(this.settings));
    this.showToast('Settings saved');
  }

  getCurrencyForCountry(country: string): string {
    const currencyMap: { [key: string]: string } = {
      'Croatia': 'EUR',
      'Germany': 'EUR',
      'Austria': 'EUR',
      'Slovenia': 'EUR'
    };
    return currencyMap[country] || 'EUR';
  }

  async clearRecentSearches() {
    const alert = await this.alertController.create({
      header: 'Clear Recent Searches',
      message: 'Are you sure you want to clear all recent searches?',
      buttons: [
        {
          text: 'Cancel',
          role: 'cancel'
        },
        {
          text: 'Clear',
          handler: () => {
            localStorage.removeItem('clothingSearchRecent');
            this.showToast('Recent searches cleared');
          }
        }
      ]
    });
    await alert.present();
  }

  async resetSettings() {
    const alert = await this.alertController.create({
      header: 'Reset Settings',
      message: 'Are you sure you want to reset all settings to default?',
      buttons: [
        {
          text: 'Cancel',
          role: 'cancel'
        },
        {
          text: 'Reset',
          handler: () => {
            localStorage.removeItem('clothingSearchSettings');
            this.settings = {
              country: 'Croatia',
              currency: 'EUR',
              clothingSize: 'M',
              shoeSize: 'EU 42',
              sizeSystem: 'EU',
              defaultSort: 'price',
              showUnavailable: false
            };
            this.showToast('Settings reset to default');
          }
        }
      ]
    });
    await alert.present();
  }

  openPrivacyPolicy() {
    // TODO: Open privacy policy
    window.open('https://yoursite.com/privacy', '_blank');
  }

  openSupport() {
    // TODO: Open support page or email
    window.open('mailto:support@clothingsearch.com', '_blank');
  }

  async showToast(message: string) {
    const toast = await this.toastController.create({
      message: message,
      duration: 2000,
      position: 'bottom',
      color: 'success'
    });
    toast.present();
  }
}
