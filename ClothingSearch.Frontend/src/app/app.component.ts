import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterOutlet } from '@angular/router';
import { IonicModule } from '@ionic/angular';
import { TranslateService } from '@ngx-translate/core';
import { UserService } from './services/user.service';

@Component({
  selector: 'app-root',
  templateUrl: 'app.component.html',
  styleUrls: ['app.component.scss'],
  standalone: true,
  imports: [CommonModule, RouterOutlet, IonicModule]
})
export class AppComponent implements OnInit {
  constructor(
    private translate: TranslateService,
    private userService: UserService
  ) {
    this.initializeApp();
  }

  ngOnInit() {}

  initializeApp() {
    this.translate.setDefaultLang('hr');
    this.translate.use('hr');
  }
}
