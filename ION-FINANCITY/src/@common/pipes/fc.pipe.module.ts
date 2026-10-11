import { NgModule } from '@angular/core';
import { FCDateFormatPipe } from './fc-date-format.pipe';
import { FCImagePipe } from './fc-image.pipe';
import { FCNamePipe } from './fc-name.pipe';
import { FCFileIconPipe } from './fc-file-icon.pipe';
import { FCFileSizePipe } from './fc-file-size.pipe';
import { FCTimeFormatPipe } from './fc-time-format.pipe';

@NgModule({
    declarations: [FCNamePipe],
    exports: [FCNamePipe],
})
export class FCNamePipeModule { }

@NgModule({
    declarations: [FCImagePipe],
    exports: [FCImagePipe],
})
export class FCImagePipeModule { }

@NgModule({
    declarations: [FCDateFormatPipe],
    exports: [FCDateFormatPipe],
})
export class FCDateFormatPipeModule { }

@NgModule({
    declarations: [FCFileIconPipe],
    exports: [FCFileIconPipe],
})
export class FCFileIconPipeModule { }

@NgModule({
    declarations: [FCFileSizePipe],
    exports: [FCFileSizePipe],
})
export class FCFileSizePipeModule { }

@NgModule({
    declarations: [FCTimeFormatPipe],
    exports: [FCTimeFormatPipe],
})
export class FCTimeFormatPipeModule { }





