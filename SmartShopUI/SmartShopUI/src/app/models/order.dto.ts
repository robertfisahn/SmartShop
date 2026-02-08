import { OrderItem } from "./orderItem.dto";

export interface Order {
  id: number;
  totalPrice: number;
  createdAt: string;
  city: string;
  street: string;
  postalCode: string;
  orderItems: OrderItem[];
}
