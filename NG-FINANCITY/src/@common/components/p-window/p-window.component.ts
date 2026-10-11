import { Component, ViewChild, ViewContainerRef, ChangeDetectorRef, OnDestroy } from '@angular/core';
import { WindowService } from '@progress/kendo-angular-dialog';

@Component({
  selector: 'p-window',
  template: `<div kendoWindowContainer #windowContainer></div>`,
  providers: [WindowService]
})
export class PWindowComponent implements OnDestroy {

  opened: boolean = true;
  @ViewChild("windowContainer", { read: ViewContainerRef, static: true }) containerRef: ViewContainerRef;

  constructor(public windowService: WindowService,
    public cd: ChangeDetectorRef) {
  }

  ngOnDestroy(): void {

    this.opened = false;
    this.containerRef.clear();
  }

  ngAfterViewInit() {

    this.cd.detectChanges();
  }
}
