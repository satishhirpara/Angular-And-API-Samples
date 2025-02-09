import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { FileService } from './_service/file.service';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent {
  title = 'UploadFilesClient1';
  private url: string = 'https://localhost:5001/api/file';
  annotationId: string = '1234567';

  constructor(private fileService: FileService)
  {

  }

  //onDownloadDispatch
  onDownload(annotationId: string) {
    this.fileService.download(annotationId)
      .subscribe(response => {
        this.fileService.SaveFile(annotationId, response);
      });
  }
  

}
