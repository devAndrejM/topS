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

export interface SearchResult {
  query: string;
  categories: CategorySummary[];
  products: Product[];
  isFromCache: boolean;
  lastUpdated: string;
  totalResults: number;
  selectedCategory: string;
}

export interface CategorySummary {
  name: string;
  count: number;
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
