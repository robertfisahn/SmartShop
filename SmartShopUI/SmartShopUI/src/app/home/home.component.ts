import { Component } from '@angular/core';
import { Router } from '@angular/router';

@Component({
    selector: 'app-home',
    templateUrl: './home.component.html',
    styleUrls: ['./home.component.css'],
    standalone: false
})
export class HomeComponent {
  title = 'SmartShopUI';
  searchQuery: string = '';

  constructor(private router: Router) { }

  onSearch(): void {
    if (this.searchQuery) {
      this.router.navigate(['/search'], { queryParams: { q: this.searchQuery } });
    }
  }
}
