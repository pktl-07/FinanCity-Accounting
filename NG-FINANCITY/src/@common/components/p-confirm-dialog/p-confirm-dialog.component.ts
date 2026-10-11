import { Component, ViewChild, ViewContainerRef, TemplateRef } from '@angular/core';
import { DialogService, DialogRef } from '@progress/kendo-angular-dialog';

@Component({
  selector: 'confirm-dialog',
  template: `<div kendoDialogContainer #dialogContainer></div>`,
  providers: [DialogService]
})
export class PConfirmDialogComponent {

  @ViewChild("dialogContainer", { read: ViewContainerRef, static: true }) public containerRef: ViewContainerRef;

  dialogOpened: boolean = false;

  constructor(private dialogService: DialogService) { }

  public open(config: { title?: string, content?: string | TemplateRef<any> | Function, action?: Array<{ text: string, primary?: boolean }> }
    = { title: 'Confirm', content: 'Are you sure you want to change.', action: [{ text: "Yes" }, { text: "No" }] }): DialogRef {

    this.dialogOpened = true;

    if (config.action == undefined)
      config.action = [{ text: "Yes" }, { text: "No" }];

    return this.dialogService.open({

      appendTo: this.containerRef,
      title: config.title,
      content: config.content,
      actions: config.action
    });
  }
}

