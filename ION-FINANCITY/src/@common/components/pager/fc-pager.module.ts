import { NgModule, CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FCPagerContainerComponent } from './fc-pager-container-component';
import { FCCardDirective } from './directives/fc-card.directive';
import { FCCardHeaderDirective } from './directives/fc-card-header.directive';
import { FCCardContentComponent } from './fc-card-content.component';
import { FCCardItemDirective } from './directives/fc-card-item.directive';
import { FCListDirective } from './directives/fc-list.directive';
import { FCListItemDirective } from './directives/fc-list-item.directive';
import { IonicModule } from '@ionic/angular';
import { FCEmptyDataTemplate } from './directives/fc-empty-data.directive';
import { FCCardHeaderTemplate } from './directives/fc-card-header-template';
import { FCCardItemTemplate } from './directives/fc-card-item-template';
import { FCCustomDataDirective } from './directives/fc-custom-data-directive';

@NgModule({
    declarations: [
        FCPagerContainerComponent,
        FCCardDirective,
        FCCardHeaderDirective,
        FCCardContentComponent,
        FCCardItemDirective,
        FCListDirective,
        FCListItemDirective,
        FCEmptyDataTemplate,
        FCCardHeaderTemplate,
        FCCardItemTemplate,
        FCCustomDataDirective
    ],
    imports: [
        CommonModule,
        IonicModule,
    ],
    exports: [
        FCPagerContainerComponent,
        FCCardDirective,
        FCCardHeaderDirective,
        FCCardContentComponent,
        FCCardItemDirective,
        FCListDirective,
        FCListItemDirective,
        FCEmptyDataTemplate,
        FCCardHeaderTemplate,
        FCCardItemTemplate,
        FCCustomDataDirective
    ],
})
export class FCPagerMoudle { }
