import { Directive, ElementRef, AfterViewInit, Input } from '@angular/core';

@Directive({
    selector: '[pInputIcon]',
})
export class PInputIconDirective implements AfterViewInit {

    @Input() public icon: string = "fa fa-lock";

    private _iconVisible: boolean = true;

    @Input()
    public get iconVisible(): boolean {
        return this._iconVisible;
    }
    public set iconVisible(value: boolean) {

        this._iconVisible = value;
        if (this._iconVisible && this.shouldWriteIcon())
            this.writeWrapperIcon();
        else
            this.removeWrapperIcon();
    }

    constructor(private _el: ElementRef) {
    }

    ngAfterViewInit(): void {

        if (this._iconVisible == true) {
            this.writeWrapperIcon();
        }
    }

    private shouldWriteIcon() {

        return !($(this._el.nativeElement).parents('.input-group').length > 0);
    }

    private writeWrapperIcon() {

        if (this._iconVisible && this.shouldWriteIcon()) {

            $(this._el.nativeElement)
                .wrap(`<div class='input-group'></div>`)
                .after(`<div class="input-group-append">
                            <span class="input-group-text">
                                <i class="${this.icon}"></i>
                            </span>
                        </div>`);
        }
    }

    private removeWrapperIcon() {
        $(this._el.nativeElement)
            .unwrap('.input-group')
            .next()
            .remove();
    }
}
