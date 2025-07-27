import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { IonicModule } from '@ionic/angular';

@Component({
  selector: 'app-explore-container',
  template: 
    <div class="container">
      <strong>{{ name }}</strong>
      <p>Explore <a target="_blank" rel="noopener noreferrer" href="https://ionicframework.com/docs/components">UI Components</a></p>
    </div>
  ,
  styleUrls: ['./explore-container.component.scss'],
  standalone: true,
  imports: [CommonModule, IonicModule]
})
export class ExploreContainerComponent {
  @Input() name?: string;
}
