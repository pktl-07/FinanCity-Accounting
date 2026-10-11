import { Component, OnDestroy, Input } from '@angular/core';
import { salesStatusColor } from 'src/@common/service/utility-service';

@Component({
    selector: 'p-sales-status',
    template: `
        <h4>
            <span class="badge badge-{{ selectedScheme.color }}" 
                style="color:var(--light);width: 100px;margin-right:20px;">
                {{ selectedScheme.text }}
            </span>
        </h4>
    `
})
export class PSalesStatusComponent implements OnDestroy {

    @Input() value: number = 0;

    public get selectedScheme(): { color: string, text: string } {

        return this.scheme[this.value];
    }

    scheme: any;

    constructor() {
        this.scheme = salesStatusColor;
    }

    ngOnDestroy(): void {
    }
}
