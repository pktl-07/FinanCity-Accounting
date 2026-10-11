import { Component, HostBinding } from '@angular/core';
@Component({
    selector: '[p-loading]', // tslint:disable-line:component-selector
    template: `
        <span class="k-loading-text">{{ loadingText }}</span>
        <div class="k-loading-image"></div>
        <div class="k-loading-color"></div>
    `
})
export class LoadingComponent {

    @HostBinding('class.k-loading-mask')
    public hostClass: boolean = true;

    public get loadingText(): string {
        return "loading...";
    }
}