import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { OrderService } from '../order.service';
import { Order } from '../order.model';

@Component({
    selector: 'app-order-create',
    templateUrl: './order-create.component.html',
    styleUrls: ['./order-create.component.css'],
    standalone: false
})
export class OrderCreateComponent implements OnInit {
    order: Order | undefined;

    constructor(
        private route: ActivatedRoute,
        private orderService: OrderService
    ) { }

    ngOnInit(): void {
        const orderId = Number(this.route.snapshot.paramMap.get('id'));
        this.orderService.getOrderById(orderId).subscribe(
            (order: Order) => {
                this.order = order;
            },
            (error) => console.error('Error fetching order details:', error)
        );
    }
}
