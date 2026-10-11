import { Injectable, ComponentFactoryResolver, ApplicationRef, Injector, EmbeddedViewRef, ComponentRef, Type, OnDestroy } from '@angular/core';
import { PWindowModule } from './p-window.module';
import { PWindowComponent } from './p-window.component';
import { WindowRef, WindowState } from '@progress/kendo-angular-dialog';
import { PageLoaderService } from '../p-page-loader/p-page-loader.service';
import { SubSink } from 'src/@common/service/utility-service';

@Injectable({ providedIn: PWindowModule })
export class PWindowComponentService implements OnDestroy {

    private _subsink: SubSink;
    public componentRef: ComponentRef<PWindowComponent>;

    constructor(public componentFactoryResolver: ComponentFactoryResolver,
        public injector: Injector,
        public appRef: ApplicationRef,
        public pageLoaderService: PageLoaderService) {

        this._subsink = new SubSink();
    }

    ngOnDestroy(): void {
        this.destroyKendoWindowComponent();
    }

    //destory view
    private destroyKendoWindowComponent() {

        try {
            this._subsink.unsubscribe();
            if (this.componentRef) {
                if (this.componentRef?.hostView)
                    this.appRef?.detachView(this.componentRef.hostView);

                this.componentRef?.destroy();
            }
            this.pageLoaderService.submitting = false;
        } catch (error) {
            //it needs to catch the exception to skip the component error.
        }
    }

    close() {
        this.destroyKendoWindowComponent();
    }

    //dynamic create view
    open<T>(config: {
        title: string,
        componentType: Type<T>,
        state?: WindowState,
        width?: number,
    }): WindowRef {

        const componentFactory = this.componentFactoryResolver.resolveComponentFactory(PWindowComponent);
        const componentRef = componentFactory.create(this.injector);
        this.appRef.attachView(componentRef.hostView);
        const domElem = (componentRef.hostView as EmbeddedViewRef<any>).rootNodes[0] as HTMLElement;
        document.body.appendChild(domElem);

        let windowRef: WindowRef;
        windowRef = componentRef.instance.windowService.open({
            title: config.title,
            content: config.componentType,
            keepContent: true,
            state: config.state == undefined ? 'default' : config.state,
            width: config.width
        });
        this._subsink.sink = windowRef.window.instance.close.subscribe(() => {
            this.destroyKendoWindowComponent();
            this.pageLoaderService.submitting = false;
        });
        this.componentRef = componentRef;
        return windowRef;
    }
}