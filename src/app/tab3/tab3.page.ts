import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { IonicModule } from '@ionic/angular';
import { TranslateModule } from '@ngx-translate/core';
import { UserService } from '../services/user.service';
import { ApiService, Country, UserSettings } from '../services/api.service';
import { ToastController, LoadingController } from '@ionic/angular';

@Component({
  selector: 'app-tab3',
  templateUrl: 'tab3.page.html',
  styleUrls: ['tab3.page.scss'],
  standalone: true,
  imports: [IonicModule, CommonModule, FormsModule, ReactiveFormsModule, TranslateModule]
})
export class Tab3Page implements OnInit {
  settingsForm: FormGroup;
  countries: Country[] = [];
  clothingSizes = ['XS', 'S', 'M', 'L', 'XL', 'XXL'];
  shoeSizeSystems = ['EU', 'US', 'UK'];
  
  euShoeSizes = Array.from({ length: 21 }, (_, i) => (35 + i).toString());
  usShoeSizes = Array.from({ length: 14 }, (_, i) => (6 + i).toString());
  ukShoeSizes = Array.from({ length: 13 }, (_, i) => (5.5 + i * 0.5).toString());

  currentUser: UserSettings | null = null;

  constructor(
    private formBuilder: FormBuilder,
    private userService: UserService,
    private apiService: ApiService,
    private toastController: ToastController,
    private loadingController: LoadingController
  ) {
    this.settingsForm = this.formBuilder.group({
      countryId: ['', Validators.required],
      clothingSize: ['M', Validators.required],
      shoeSize: ['42', Validators.required],
      shoeSizeSystem: ['EU', Validators.required]
    });
  }

  ngOnInit() {
    this.loadCountries();
    this.loadUserSettings();
  }

  async loadCountries() {
    try {
      this.countries = await this.apiService.getCountries().toPromise() || [];
    } catch (error) {
      console.error('Error loading countries:', error);
      this.showToast('Greška pri dohvaćanju zemalja', 'danger');
    }
  }

  loadUserSettings() {
    this.userService.currentUser$.subscribe(user => {
      this.currentUser = user;
      if (user) {
        this.settingsForm.patchValue({
          countryId: user.countryId,
          clothingSize: user.clothingSize,
          shoeSize: user.shoeSize,
          shoeSizeSystem: user.shoeSizeSystem
        });
      }
    });
  }

  get availableShoeSizes(): string[] {
    const system = this.settingsForm.get('shoeSizeSystem')?.value;
    switch (system) {
      case 'US': return this.usShoeSizes;
      case 'UK': return this.ukShoeSizes;
      default: return this.euShoeSizes;
    }
  }

  onShoeSizeSystemChange() {
    this.settingsForm.patchValue({
      shoeSize: this.availableShoeSizes[5]
    });
  }

  async saveSettings() {
    if (this.settingsForm.valid) {
      const loading = await this.loadingController.create({
        message: 'Spremam postavke...'
      });
      await loading.present();

      try {
        const settings = await this.userService.updateSettings(this.settingsForm.value).toPromise();
        if (settings) {
          this.showToast('Postavke uspješno spremljene!', 'success');
          this.userService.refreshUserSettings();
        }
      } catch (error) {
        console.error('Error saving settings:', error);
        this.showToast('Greška pri spremanju postavki', 'danger');
      } finally {
        await loading.dismiss();
      }
    } else {
      this.showToast('Molimo ispunite sva polja', 'warning');
    }
  }

  private async showToast(message: string, color: string = 'primary') {
    const toast = await this.toastController.create({
      message,
      duration: 3000,
      color,
      position: 'top'
    });
    await toast.present();
  }
}
