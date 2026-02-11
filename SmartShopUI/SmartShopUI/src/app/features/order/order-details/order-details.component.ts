import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { OrderService } from '../order.service';

@Component({
    selector: 'app-order-details',
    templateUrl: './order-details.component.html',
    styleUrls: ['./order-details.component.css'],
    standalone: false
})
export class OrderDetailsComponent implements OnInit {
    orderId: number = 0;
    orderDetails: any;

    constructor(private route: ActivatedRoute, private orderService: OrderService) { }

    ngOnInit(): void {
        this.orderId = Number(this.route.snapshot.paramMap.get('orderId'));
        this.loadOrderDetails();
    }

    loadOrderDetails(): void {
        this.orderService.getOrderById(this.orderId).subscribe({
            next: (details) => {
                this.orderDetails = details;
            },
            error: (err) => {
                console.error('Error loading order details:', err);
            }
        });
    }
}
