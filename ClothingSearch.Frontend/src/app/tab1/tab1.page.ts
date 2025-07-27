import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { IonicModule } from '@ionic/angular';
import { TranslateModule } from '@ngx-translate/core';
import { ApiService, SearchResult, CategorySummary, Product } from '../services/api.service';
import { UserService } from '../services/user.service';
import { LoadingController, ToastController } from '@ionic/angular';

@Component({
  selector: 'app-tab1',
  templateUrl: 'tab1.page.html',
  styleUrls: ['tab1.page.scss'],
  standalone: true,
  imports: [IonicModule, CommonModule, FormsModule, TranslateModule]
})
export class Tab1Page implements OnInit {
  searchQuery: string = '';
  searchResults: SearchResult | null = null;
  selectedCategory: string = '';
  isLoading: boolean = false;

  constructor(
    private apiService: ApiService,
    private userService: UserService,
    private loadingController: LoadingController,
    private toastController: ToastController
  ) {}

  ngOnInit() {}

  async onSearch() {
    if (!this.searchQuery.trim()) {
      return;
    }

    this.isLoading = true;
    const loading = await this.loadingController.create({
      message: 'Pretražujem...'
    });
    await loading.present();

    try {
      const userId = this.userService.getCurrentUserId();
      this.searchResults = await this.apiService.search(
        this.searchQuery, 
        userId, 
        this.selectedCategory
      ).toPromise() || null;
    } catch (error) {
      console.error('Search error:', error);
      this.showToast('Greška pri pretraživanju', 'danger');
    } finally {
      this.isLoading = false;
      await loading.dismiss();
    }
  }

  onCategorySelect(category: string) {
    this.selectedCategory = category;
    this.onSearch();
  }

  clearCategory() {
    this.selectedCategory = '';
    this.onSearch();
  }

  openProduct(product: Product) {
    window.open(product.affiliateUrl, '_blank');
  }

  private async showToast(message: string, color: string = 'primary') {
    const toast = await this.toastController.create({
      message,
      duration: 2000,
      color
    });
    await toast.present();
  }

  formatPrice(price: number, currency: string): string {
    return new Intl.NumberFormat('hr-HR', {
      style: 'currency',
      currency: currency === 'HRK' ? 'HRK' : currency
    }).format(price);
  }

  getTimeAgo(dateString: string): string {
    const date = new Date(dateString);
    const now = new Date();
    const diffInHours = Math.floor((now.getTime() - date.getTime()) / (1000 * 60 * 60));
    
    if (diffInHours < 1) {
      return 'prije manje od sat vremena';
    } else if (diffInHours === 1) {
      return 'prije sat vremena';
    } else {
      return "prije  sati";
    }
  }
}
