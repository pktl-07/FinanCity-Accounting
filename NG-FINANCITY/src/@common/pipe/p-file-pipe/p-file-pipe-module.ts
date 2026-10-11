import { NgModule } from '@angular/core';
import { PFileIcon } from './p-file-icon.pipe';
import { PFileSizePipe } from './p-file-size-pipe';

const pipes = [PFileSizePipe,
    PFileIcon
]

@NgModule({
    declarations: [pipes],
    imports: [

    ],
    exports: [pipes]
})
export class PFilePipeModule {
}



