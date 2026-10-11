import { NgModule } from '@angular/core';
import { PSalespersonNamePipe } from './p-salesperson-name-pipe';
import { PSalespersonImagePipe } from './p-salesperson-image-pipe';

const pipes = [PSalespersonNamePipe,
    PSalespersonImagePipe]

@NgModule({
    declarations: [pipes],
    imports: [

    ],
    exports: [pipes]
})
export class PSalespersonPipeModule {
}



