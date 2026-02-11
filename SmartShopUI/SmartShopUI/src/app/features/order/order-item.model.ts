import { ProductDto } from "../product/product.model";

export interface OrderItem {
    id: number;
    quantity: number;
    productId: number;
    product: ProductDto;
}
