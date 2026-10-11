import { Directive, ElementRef, ViewContainerRef } from '@angular/core';

@Directive({
    selector: '[popupAnchor]',
    exportAs: 'popupAnchor'
})
export class GridValidationPopupAnchor {
    constructor(public element: ElementRef) { }
}