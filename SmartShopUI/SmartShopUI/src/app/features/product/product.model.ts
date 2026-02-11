export interface ProductDto {
    id: number;
    name: string;
    description: string;
    price: number;
    categoryId: number;
    stockQuantity: number;
    imagePath: string;
    image?: File
}
