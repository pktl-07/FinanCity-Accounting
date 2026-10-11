import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { PopupModule } from '@progress/kendo-angular-popup';
import { PNotificationPopupComponent } from './p-notification-popup.component';
import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { NotificationContentTemplateDirective } from './p-notification-popup-content-template.directive';
import { PNotificationMessageTemplateComponent } from './p-notification-message-template.component';
import { LayoutModule } from '@progress/kendo-angular-layout';
import { NotificationBodyTemplateDirective } from './p-notification-body-template.directive';
import { NotificationHeaderTemplateDirective } from './p-notification-header-template.directive';
import { NotificationActionTemplateDirective } from './p-notification-action-template.directive';

const components = [PNotificationPopupComponent, PNotificationMessageTemplateComponent];
const directives = [NotificationContentTemplateDirective,
  NotificationBodyTemplateDirective,
  NotificationHeaderTemplateDirective,
  NotificationActionTemplateDirective];

@NgModule({
  entryComponents: [
    ...components
  ],
  declarations: [
    ...components,
    ...directives
  ],
  imports: [
    CommonModule,
    PopupModule,
    BrowserModule,
    BrowserAnimationsModule,
    LayoutModule,
  ],
  exports: [
    ...components,
    ...directives
  ]
})
export class PNotificationPopupModule { }
