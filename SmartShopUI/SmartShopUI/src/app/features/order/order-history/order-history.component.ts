import { Component, OnInit } from '@angular/core';
import { OrderService } from '../order.service';

@Component({
    selector: 'app-order-history',
    templateUrl: './order-history.component.html',
    styleUrls: ['./order-history.component.css'],
    standalone: false
})
export class OrderHistoryComponent implements OnInit {
    orders: any[] = [];
    loading: boolean = false;
    error: string | null = null;

    constructor(private orderService: OrderService) { }

    ngOnInit(): void {
        this.loadOrders();
    }

    loadOrders(): void {
        this.loading = true;
        this.error = null;
        this.orderService.getUserOrders().subscribe({
            next: (orders) => {
                this.orders = orders.map(order => ({
                    ...order,
                    showMore: false
                }));
                this.loading = false;
            },
            error: (err) => {
                this.error = 'Error loading orders: ' + err.message;
                this.loading = false;
                console.error('Error loading orders:', err);
            }
        });
    }

    toggleOrderItems(selectedOrder: any): void {
        this.orders.forEach(order => {
            if (order === selectedOrder) {
                order.showMore = !order.showMore;
            } else {
                order.showMore = false;
            }
        });
    }
}
