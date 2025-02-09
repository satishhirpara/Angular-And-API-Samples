import { HttpClient, HttpResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class FileService {
  private url: string = 'https://localhost:5001/api/file';

  constructor(private http: HttpClient) { }

  public upload(formData: FormData) {
    return this.http.post(`${this.url}/upload`, formData, {
        reportProgress: true,
        observe: 'events',
    });
  }

  public download(annotationId: string) { 
    //console.log(annotationId);
    return this.http.get(`${this.url}/download`, {
      observe: 'response',
      responseType: 'blob'
    }); 
  }

  public SaveFile(annotationId: string, response: HttpResponse<Blob> | null)
  {
    if(response != null && response.body != null)  
    {
      // get file name from Content-Disposition
      var contentDisposition = response.headers.get('Content-Disposition');
      var fileName = this.getFilenameFromContentDisposition(contentDisposition);
      if(fileName === null || fileName === "")
      {
        fileName = annotationId;
      }

      //Save file
      const blob = new Blob([response.body], { type: response.body.type });
      const url = window.URL.createObjectURL(blob);
      const a = document.createElement('a');
      a.href = url;
      a.download = fileName;
      document.body.appendChild(a);
      a.click();
      document.body.removeChild(a);
    }

  }

  private getFilenameFromContentDisposition(contentDisposition: string | null): string 
  {
    var filename = "";
    if (contentDisposition && contentDisposition.indexOf('attachment') !== -1) {
        var filenameRegex = /filename[^;=\n]*=((['"]).*?\2|[^;\n]*)/;
        var matches = filenameRegex.exec(contentDisposition);
        if (matches != null && matches[1]) filename = matches[1].replace(/['"]/g, '');
    }
    return filename;
  }



  public getPhotos() { 
    return this.http.get(`${this.url}/getPhotos`); 
  }
}
