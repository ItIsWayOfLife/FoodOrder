import { Component, OnInit, Output, EventEmitter } from '@angular/core';
import { HttpEventType, HttpClient } from '@angular/common/http';
import { Input} from '@angular/core';

@Component({
  selector: 'app-upload',
  templateUrl: './upload.component.html',
  styleUrls: ['./upload.component.css']
})
export class UploadComponent implements OnInit {
  public progress: number;
  public message: string;

  @Output() public onUploadFinished = new EventEmitter();

  @Output() notify: EventEmitter<string> = new EventEmitter<string>();

  @Input() fileNameR: string;

  isAddImg: boolean;
  urlImage: string;
  _fileName: string;
  baseURLImage: string;

  constructor(private http: HttpClient) {
    this.isAddImg = false;
    this.urlImage = "";
    this._fileName = "";
    this.baseURLImage = "https://localhost:44342/files/images/";
  }

  ngOnInit() {
  }

  showImg() {
    this.isAddImg = !this.isAddImg;
    if (this.urlImage == "") {
      this.urlImage = this.fileNameR;
      this._fileName = "Image";
    }
  }

  public uploadFile = (files) => {
    if (files.length === 0) {
      return;
    }

    let fileToUpload = <File>files[0];
    const formData = new FormData();
    formData.append('file', fileToUpload, fileToUpload.name);
    this._fileName = fileToUpload.name;

    this.http.post('https://localhost:44342/api/upload', formData, { reportProgress: true, observe: 'events' })
      .subscribe(event => {
        if (event.type === HttpEventType.UploadProgress)
          this.progress = Math.round(100 * event.loaded / event.total);
        else if (event.type === HttpEventType.Response) {
          this.message = 'Upload successful.';
          this.onUploadFinished.emit(event.body);
        }
      });
    this.urlImage = this.baseURLImage + this._fileName;

    this.notify.emit(this._fileName);
  }
}
