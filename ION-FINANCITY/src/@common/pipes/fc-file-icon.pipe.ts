import { Pipe, PipeTransform } from '@angular/core';
import {
    faFilePdf,
    faFileWord,
    faFileImage,
    faFilePowerpoint,
    faFileAlt,
    faFileExcel,
    faFileAudio,
    faFileVideo,
    faFileCsv,
    faFileArchive,
    faFileCode,
    faFile
} from '@fortawesome/pro-regular-svg-icons';

@Pipe({
    name: 'fcFileIcon',
})
export class FCFileIconPipe implements PipeTransform {

    transform(value: string, ...args: any[]): any {

        value = value.toLowerCase();

        const icons = {
            ['pdf']: faFilePdf,
            ['doc']: faFileWord,
            ['image']: faFileImage,
            ['ppt']: faFilePowerpoint,
            ['txt']: faFileAlt,
            ['xls']: faFileExcel,
            ['cvs']: faFileCsv,
            ['audio']: faFileAudio,
            ['mov']: faFileVideo,
            ['zip']: faFileArchive,
            ['htm']: faFileCode,
            ['file']: faFile,
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