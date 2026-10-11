import { Component, OnDestroy, Input } from '@angular/core';
import { recordStatusColor } from 'src/@common/service/utility-service';

@Component({
    selector: 'p-recordstatus',
    template: `
        <h4>
            <span class="badge badge-{{ selectedScheme.color }}" 
                style="color:var(--light);width: 100px;">
                {{ selectedScheme.text }}
            </span>
        </h4>
    `
})
export class PRecordStatusComponent implements OnDestroy {

    @Input() value: number = 0;

    public get selectedScheme(): { color: string, text: string } {
        return this.scheme[this.value];
    }

    scheme: any;

    constructor() {

        this.scheme = recordStatusColor;
    }

    ngOnDestroy(): void {
    }
}
