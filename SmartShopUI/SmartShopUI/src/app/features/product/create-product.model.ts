export interface CreateProductDto {
    name: string;
    description: string;
    price: number;
    stockQuantity: number;
    categoryId: number;
    image?: File;
}
