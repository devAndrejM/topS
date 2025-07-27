import { Component, OnInit, ViewChild } from '@angular/core';
import { IonSearchbar } from '@ionic/angular';
import { Router } from '@angular/router';

export interface SearchResult {
  id: string;
  name: string;
  variant: string;
  size: string;
  store: string;
  price: number;
  shipping: number;
  totalPrice: number;
  inStock: boolean;
  delivery: string;
  category: string;
  isBestDeal: boolean;
  url: string;
  imageUrl?: string;
}

export interface Category {
  key: string;
  name: string;
  count: number;
}

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
  selector: 'app-search',
  templateUrl: './search.page.html',
  styleUrls: ['./search.page.scss'],
})
export class SearchPage implements OnInit {
  @ViewChild('searchBar', { static: false }) searchBar!: IonSearchbar;

  // Search state
  searchTerm: string = '';
  searchResults: SearchResult[] = [];
  filteredResults: SearchResult[] = [];
  isLoading: boolean = false;
  hasSearched: boolean = false;

  // Filtering and sorting
  selectedCategory: string = 'all';
  sortBy: string = 'price';
  availableCategories: Category[] = [];

  // Recent searches
  recentSearches: string[] = [];

  // User settings (from localStorage or service)
  userSettings: UserSettings = {
    country: 'Croatia',
    currency: 'EUR',
    clothingSize: 'M',
    shoeSize: 'EU 42',
    sizeSystem: 'EU',
    defaultSort: 'price',
    showUnavailable: false
  };

  constructor(private router: Router) {}

  ngOnInit() {
    this.loadUserSettings();
    this.loadRecentSearches();
    
    // Set default sort from settings
    this.sortBy = this.userSettings.defaultSort;
  }

  // Search functionality
  onSearchInput(event: any) {
    const query = event.target.value;
    if (query && query.trim().length > 2) {
      // Debounce search
      setTimeout(() => {
        if (this.searchTerm === query) {
          this.performSearch();
        }
      }, 300);
    }
  }

  performSearch() {
    if (!this.searchTerm || this.searchTerm.trim().length < 2) {
      return;
    }

    this.isLoading = true;
    this.hasSearched = true;

    // Add to recent searches
    this.addToRecentSearches(this.searchTerm);

    // Simulate API call with mock data
    setTimeout(() => {
      this.searchResults = this.getMockSearchResults();
      this.processSearchResults();
      this.isLoading = false;
    }, 1000);

    // TODO: Replace with actual API call
    // this.searchService.search(this.searchTerm).subscribe(results => {
    //   this.searchResults = results;
    //   this.processSearchResults();
    //   this.isLoading = false;
    // });
  }

  clearSearch() {
    this.searchTerm = '';
    this.searchResults = [];
    this.filteredResults = [];
    this.hasSearched = false;
    this.selectedCategory = 'all';
  }

  // Process search results
  processSearchResults() {
    // Calculate total prices
    this.searchResults.forEach(result => {
      result.totalPrice = result.price + result.shipping;
    });

    // Find best deal
    if (this.searchResults.length > 0) {
      const minPrice = Math.min(...this.searchResults.map(r => r.totalPrice));
      this.searchResults.forEach(result => {
        result.isBestDeal = result.totalPrice === minPrice;
      });
    }

    // Generate category filters
    this.generateCategoryFilters();

    // Apply initial filtering and sorting
    this.applyFiltersAndSort();
  }

  generateCategoryFilters() {
    const categoryMap = new Map<string, number>();
    
    // Count items in each category
    this.searchResults.forEach(result => {
      const count = categoryMap.get(result.category) || 0;
      categoryMap.set(result.category, count + 1);
    });

    // Create category filter options
    this.availableCategories = [
      { key: 'all', name: 'All', count: this.searchResults.length }
    ];

    categoryMap.forEach((count, category) => {
      this.availableCategories.push({
        key: category,
        name: category,
        count: count
      });
    });
  }

  // Category filtering
  selectCategory(category: string) {
    this.selectedCategory = category;
    this.applyFiltersAndSort();
  }

  // Sorting
  sortResults() {
    this.applyFiltersAndSort();
  }

  applyFiltersAndSort() {
    // Filter by category
    this.filteredResults = this.selectedCategory === 'all' 
      ? [...this.searchResults]
      : this.searchResults.filter(result => result.category === this.selectedCategory);

    // Filter out unavailable sizes if setting is disabled
    if (!this.userSettings.showUnavailable) {
      this.filteredResults = this.filteredResults.filter(result => result.inStock);
    }

    // Sort results
    this.filteredResults.sort((a, b) => {
      switch (this.sortBy) {
        case 'price':
          return a.totalPrice - b.totalPrice;
        case 'delivery':
          // Simple delivery speed sorting (you can make this more sophisticated)
          return a.delivery.localeCompare(b.delivery);
        case 'relevance':
        default:
          // Sort by relevance (best deals first, then alphabetically)
          if (a.isBestDeal && !b.isBestDeal) return -1;
          if (!a.isBestDeal && b.isBestDeal) return 1;
          return a.name.localeCompare(b.name);
      }
    });
  }

  // Open store
  openStore(result: SearchResult) {
    // Animate the click
    const element = event?.currentTarget as HTMLElement;
    if (element) {
      element.style.transform = 'scale(0.98)';
      setTimeout(() => {
        element.style.transform = '';
      }, 150);
    }

    // Open external store URL
    // TODO: Add analytics tracking
    window.open(result.url, '_blank');
  }

  // Recent searches management
  loadRecentSearches() {
    const stored = localStorage.getItem('clothingSearchRecent');
    if (stored) {
      this.recentSearches = JSON.parse(stored);
    }
  }

  addToRecentSearches(term: string) {
    const cleanTerm = term.trim().toLowerCase();
    
    // Remove if already exists
    this.recentSearches = this.recentSearches.filter(t => t.toLowerCase() !== cleanTerm);
    
    // Add to beginning
    this.recentSearches.unshift(term.trim());
    
    // Keep only last 5
    this.recentSearches = this.recentSearches.slice(0, 5);
    
    // Save to localStorage
    localStorage.setItem('clothingSearchRecent', JSON.stringify(this.recentSearches));
  }

  // User settings
  loadUserSettings() {
    const stored = localStorage.getItem('clothingSearchSettings');
    if (stored) {
      this.userSettings = { ...this.userSettings, ...JSON.parse(stored) };
    }
  }

  // Mock data - Replace with actual API integration
  getMockSearchResults(): SearchResult[] {
    const baseResults = [
      {
        id: '1',
        name: 'Nike Air Max 270',
        variant: 'Black/White',
        size: this.userSettings.shoeSize,
        store: 'Amazon.de',
        price: 89.99,
        shipping: 0,
        totalPrice: 0, // Will be calculated
        inStock: true,
        delivery: 'Prime 2-day delivery',
        category: 'Shoes',
        isBestDeal: false, // Will be calculated
        url: 'https://amazon.de/nike-air-max-270',
        imageUrl: 'assets/images/nike-air-max-270.jpg'
      },
      {
        id: '2',
        name: 'Nike Air Max 270',
        variant: 'Black/White',
        size: this.userSettings.shoeSize,
        store: 'Zalando',
        price: 92.50,
        shipping: 0,
        totalPrice: 0,
        inStock: true,
        delivery: '1-2 day delivery',
        category: 'Shoes',
        isBestDeal: false,
        url: 'https://zalando.com/nike-air-max-270',
        imageUrl: 'assets/images/nike-air-max-270.jpg'
      },
      {
        id: '3',
        name: 'Nike Air Max 270',
        variant: 'Black/White',
        size: this.userSettings.shoeSize,
        store: 'Hervis.hr',
        price: 95.00,
        shipping: 4.99,
        totalPrice: 0,
        inStock: true,
        delivery: '3-5 day delivery',
        category: 'Shoes',
        isBestDeal: false,
        url: 'https://hervis.hr/nike-air-max-270',
        imageUrl: 'assets/images/nike-air-max-270.jpg'
      },
      {
        id: '4',
        name: 'Nike Dri-FIT T-Shirt',
        variant: 'Black',
        size: this.userSettings.clothingSize,
        store: 'Nike.com',
        price: 24.99,
        shipping: 0,
        totalPrice: 0,
        inStock: true,
        delivery: 'Free delivery',
        category: 'Clothing',
        isBestDeal: false,
        url: 'https://nike.com/dri-fit-tshirt',
        imageUrl: 'assets/images/nike-tshirt.jpg'
      },
      {
        id: '5',
        name: 'Nike Dri-FIT T-Shirt',
        variant: 'Black',
        size: this.userSettings.clothingSize,
        store: 'Decathlon',
        price: 19.99,
        shipping: 2.99,
        totalPrice: 0,
        inStock: true,
        delivery: '2-3 day delivery',
        category: 'Clothing',
        isBestDeal: false,
        url: 'https://decathlon.hr/nike-tshirt',
        imageUrl: 'assets/images/nike-tshirt.jpg'
      },
      {
        id: '6',
        name: 'Nike Dri-FIT T-Shirt',
        variant: 'Black',
        size: this.userSettings.clothingSize,
        store: 'SportVision',
        price: 27.99,
        shipping: 0,
        totalPrice: 0,
        inStock: false,
        delivery: 'Out of stock',
        category: 'Clothing',
        isBestDeal: false,
        url: 'https://sportvision.hr/nike-tshirt',
        imageUrl: 'assets/images/nike-tshirt.jpg'
      }
    ];

    // Filter based on search term relevance
    return baseResults.filter(result => 
      result.name.toLowerCase().includes(this.searchTerm.toLowerCase()) ||
      result.category.toLowerCase().includes(this.searchTerm.toLowerCase())
    );
  }
}
