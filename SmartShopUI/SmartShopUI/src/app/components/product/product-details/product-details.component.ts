import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ProductService } from '../../../services/product/product.service';
import { ProductDto } from '../../../models/product.dto';
import { environment } from 'src/environments/environment';

@Component({
    selector: 'app-product-details',
    templateUrl: './product-details.component.html',
    styleUrls: ['./product-details.component.css'],
    standalone: false
})
export class ProductDetailsComponent implements OnInit {
  product: ProductDto | null = null;

  constructor(
    private route: ActivatedRoute,
    private productService: ProductService
  ) { }

  ngOnInit(): void {
    this.getProductDetails();
  }

  getProductDetails(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.productService.getProductById(+id).subscribe((data: ProductDto) => {
        this.product = data;
      });
    }
  }

  getImageUrl(imagePath: string): string {
    return this.productService.getImageUrl(imagePath);
  }
}
