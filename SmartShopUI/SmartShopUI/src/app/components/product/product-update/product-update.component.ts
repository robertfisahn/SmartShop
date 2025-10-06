import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ProductService } from '../../../services/product/product.service';
import { CategoryService } from '../../../services/category/category.service';
import { CreateProductDto } from '../../../models/createProduct.dto';
import { ProductDto } from '../../../models/product.dto';

@Component({
    selector: 'app-product-update',
    templateUrl: './product-update.component.html',
    styleUrls: ['./product-update.component.css'],
    standalone: false
})
export class ProductUpdateComponent implements OnInit {
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
  categories: any[] = [];
  selectedFile: File | null = null;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private productService: ProductService,
    private categoryService: CategoryService
  ) { }

  ngOnInit(): void {
    this.productId = Number(this.route.snapshot.paramMap.get('productId'));

    this.loadProduct();

    this.loadCategories();
  }

  loadProduct(): void {
    this.productService.getProductById(this.productId).subscribe(
      (data: ProductDto) => {
        this.product = data;
      },
      error => {
        console.error('Error loading product', error);
      }
    );
  }

  loadCategories(): void {
    this.categoryService.getAllCategories().subscribe((data: any[]) => {
      this.categories = data;
    });
  }

  onFileSelected(event: any): void {
    this.selectedFile = event.target.files[0];
  }

  onSubmit(): void {
    const formData = new FormData();
    formData.append('name', this.product.name);
    formData.append('description', this.product.description);
    formData.append('price', this.product.price.toString());
    formData.append('stockQuantity', this.product.stockQuantity.toString());
    formData.append('categoryId', this.product.categoryId.toString());

    if (this.selectedFile) {
      formData.append('file', this.selectedFile);
    }
    
    this.productService.updateProduct(this.productId, formData).subscribe(
      response => {
        console.log('Product updated!', response);
        this.router.navigate(['/product-details', this.productId]);
      },
      error => {
        console.error('Error updating product', error); 
      }
    );
  }

  getImageUrl(imagePath: string): string {
    return this.productService.getImageUrl(imagePath);
  }
}
