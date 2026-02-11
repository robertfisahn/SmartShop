import { Component, OnInit } from '@angular/core';
import { AbstractControl, FormBuilder, FormGroup, Validators, ValidationErrors } from '@angular/forms';
import { Router } from '@angular/router';
import { ProductService } from '../product.service';
import { CategoryService } from '../../category/category.service';
import { of, map, catchError, Observable, tap } from 'rxjs';

@Component({
    selector: 'app-product-create',
    templateUrl: './product-create.component.html',
    styleUrls: ['./product-create.component.css'],
    standalone: false
})
export class ProductCreateComponent implements OnInit {
    productForm!: FormGroup;
    categories: any[] = [];
    selectedFile: File | null = null;
    backendErrors: any = {};
    fileError: string | null = null;

    constructor(
        private fb: FormBuilder,
        private productService: ProductService,
        private categoryService: CategoryService,
        private router: Router
    ) { }

    ngOnInit(): void {
        this.loadCategories();
        this.createForm();
    }

    createForm(): void {
        this.productForm = this.fb.group({
            name: ['', {
                validators: [Validators.required],
                asyncValidators: [this.productNameAvailable.bind(this)],
                updateOn: 'blur'
            }],
            description: ['', {
                validators: [Validators.required, this.descriptionValidator],
                updateOn: 'blur'
            }],
            price: ['', {
                validators: [Validators.required, this.priceValidator],
                updateOn: 'blur'
            }],
            stockQuantity: ['', {
                validators: [Validators.required, this.quantityValidator],
                updateOn: 'blur'
            }],
            categoryId: [null, [Validators.required]]
        });
    }


    loadCategories(): void {
        this.categoryService.getAllCategories().subscribe((data: any[]) => {
            this.categories = data;
        });
    }

    onFileSelected(event: any): void {
        const file: File = event.target.files[0];

        if (file) {
            const validTypes = ['image/jpeg', 'image/png'];

            if (!validTypes.includes(file.type)) {
                this.fileError = 'Invalid file type. Please select a JPG or PNG image.';
                this.selectedFile = null;
            } else {
                this.fileError = null;
                this.selectedFile = file;
            }
        }
    }

    onSubmit(): void {
        if (this.productForm.invalid) {
            return;
        }

        const formData = new FormData();
        formData.append('name', this.productForm.get('name')?.value);
        formData.append('description', this.productForm.get('description')?.value);
        formData.append('price', this.productForm.get('price')?.value);
        formData.append('stockQuantity', this.productForm.get('stockQuantity')?.value);
        formData.append('categoryId', this.productForm.get('categoryId')?.value);

        if (this.selectedFile) {
            formData.append('file', this.selectedFile);
        }

        this.productService.createProduct(formData).subscribe(
            response => {
                console.log('Product created!', response);
                this.router.navigate(['/product/all']);
            },
            error => {
                if (error.status === 400 && error.error.errors) {
                    this.backendErrors = error.error.errors;
                }
            }
        );
    }

    productNameAvailable(control: AbstractControl): Observable<any> {
        if (!control.value) {
            return of(null);
        }

        return this.productService.checkProductName(control.value).pipe(
            tap(() => {
                this.clearBackendError();
            }),
            map((isAvailable: boolean) => {
                return isAvailable ? null : { productNameTaken: true };
            }),
            catchError((error) => {
                this.backendErrors['name'] = error.message;
                return of({ backendError: true });
            })
        );
    }

    clearBackendError(): void {
        if (this.backendErrors['name']) {
            delete this.backendErrors['name'];
        }
    }

    priceValidator(control: AbstractControl): ValidationErrors | null {
        const value = control.value;
        const regex = /^\d{1,8}(\.\d{1,2})?$/;

        if (value <= 0 || value > 99999999.99 || !regex.test(value)) {
            return {
                invalidPrice: 'Price must be between 0.01 and 99999999.99 and have no more than 2 decimal places'
            };
        }

        return null;
    }

    quantityValidator(control: AbstractControl): ValidationErrors | null {
        const value = control.value;

        if (value <= 0 || value > 9999) {
            return {
                invalidQuantity: 'Quantity must be between 1 and 9999'
            };
        }

        return null;
    }

    descriptionValidator(control: AbstractControl): ValidationErrors | null {
        const value = control.value || '';

        if (value.length < 10 || value.length > 2000) {
            return { invalidDescription: 'Description must be between 10 and 2000 characters.' };
        }

        return null;
    }
}
