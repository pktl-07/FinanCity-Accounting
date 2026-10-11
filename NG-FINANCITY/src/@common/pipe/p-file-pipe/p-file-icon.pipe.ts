import { Pipe, PipeTransform, OnDestroy } from '@angular/core';

@Pipe({
    name: 'pFileIcon',
})
export class PFileIcon implements PipeTransform, OnDestroy {


    constructor() {
    }

    ngOnDestroy(): void {
    }

    transform(value: string, ...args: any[]): any {

        value = value.toLowerCase();

        const icons = {
            ['pdf']: 'far fa-file-pdf',
            ['doc']: 'far fa-file-word',
            ['image']: 'far fa-file-image',
            ['ppt']: 'far fa-file-powerpoint',
            ['txt']: 'far fa-file-alt',
            ['xls']: 'far fa-file-excel',
            ['audio']: 'far fa-file-audio',
            ['mov']: 'far fa-file-video',
            ['zip']: 'far fa-file-archive',
            ['htm']: 'far fa-file-code',
            ['file']: 'far fa-file',
        }

        const fname = value.split('.');
        const extension = fname[fname.length - 1];
        switch (extension) {
            case 'pdf': return icons.pdf;

            case 'doc':
            case 'docx':
                return icons.doc;


            case 'gif':
            case 'png':
            case 'jpg':
                return icons.image;

            case 'txt':
            case 'ini':
            case 'csv':
            case 'java':
                return icons.txt;

            case 'xls':
            case 'xlsx':
                return icons.xls;

            case 'htm':
            case 'html':
            case 'php':
            case 'js':
            case 'css':
            case 'html':
                return icons.htm;

            case 'mp3':
            case 'wav':
                return icons.audio;

            case 'ppt':
            case 'pptx':
                return icons.ppt;

            case 'zip':
            case 'rar':
            case 'tar':
            case 'gzip':
            case 'gz':
            case '7z':
                return icons.zip;

            case 'avi':
            case 'mpg':
            case 'mkv':
            case 'mov':
            case 'mp4':
            case '3gp':
            case 'webm':
            case 'wmv':
                return icons.mov;

            default: return icons.file;
        }
    }
}