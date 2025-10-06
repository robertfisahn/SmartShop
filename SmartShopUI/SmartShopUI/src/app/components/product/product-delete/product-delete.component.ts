import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ProductService } from '../../../services/product/product.service';
import { ProductDto } from '../../../models/product.dto';

@Component({
    selector: 'app-product-delete',
    templateUrl: './product-delete.component.html',
    styleUrls: ['./product-delete.component.css'],
    standalone: false
})
export class ProductDeleteComponent implements OnInit {
  productId!: number;
  product: ProductDto = {
    id: 0,
    name: '',
    description: '',
    price: 0,
    stockQuantity: 0,
    categoryId: 0,
    imagePath: ''
  };

  constructor(
    private route: ActivatedRoute,
    private productService: ProductService,
    private router: Router
  ) { }

  ngOnInit(): void {
    this.productId = Number(this.route.snapshot.paramMap.get('productId'));
    if (this.productId) {
      this.loadProduct(this.productId);
    }
  }

  loadProduct(id: number): void {
    this.productService.getProductById(id).subscribe(
      (product: ProductDto) => {
        this.product = product;
      },
      (error) => {
        console.error('Error loading product', error);
        this.router.navigate(['/product/all']);
      }
    );
  }

  onDeleteConfirm(): void {
    if (this.productId) {
      this.productService.deleteProduct(this.productId).subscribe(
        () => {
          console.log('Product deleted successfully');
          this.router.navigate(['/product/all']);
        },
        (error) => {
          console.error('Error deleting product', error);
        }
      );
    }
  }

  onCancel(): void {
    this.router.navigate(['/product/all']);
  }
}
