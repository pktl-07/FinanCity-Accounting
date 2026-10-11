import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { IonicModule } from '@ionic/angular';
import { FCImageLoaderPage } from './fc-image-loader.page';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';

@NgModule({
  declarations: [FCImageLoaderPage],
  imports: [
    IonicModule,
    ReactiveFormsModule,
    FormsModule,
    CommonModule,
  ],
  exports: [FCImageLoaderPage]
})
export class FCImageLoaderModule { }
