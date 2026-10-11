import { Component, Input } from '@angular/core';

@Component({
    selector: 'fc-card-content',
    template: `
    <ion-card-content>
        {{content}}    
    </ion-card-content>
    `
})
export class FCCardContentComponent {

    @Input() content: string;
}
