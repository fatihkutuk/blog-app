import { Injectable } from '@angular/core';
import {HttpClient} from '@angular/common/http';
import { ArticlePagination } from '../models/article-pagination';
import {tap} from 'rxjs/operators';
import { Article } from '../models/article';
@Injectable({
  providedIn: 'root'
})
export class ArticleService {
  constructor( private httpClient:HttpClient) { }
  public loading:boolean=true;
  private apiUrl = "https://localhost:5001/api/articles";

  getArticles(page:number,pageSize:number)
  {
    let api = `${this.apiUrl}/${page}/${pageSize}`;
    console.log(api);
    return this.httpClient.get<ArticlePagination>(api).pipe(tap(x=> {
      this.loading = false ;
    }));
  }

  getArticle(id:number)
  {
     let api = `${this.apiUrl}/${id}`;
     return this.httpClient.get<Article>(api).pipe(tap(x=>{
       this.loading =false;
     }));
  }

  getArticleWithCategory(categoryId:number,page:number,pageSize:number)
  {
    let api = `${this.apiUrl}/GetArticleWithCategory/${categoryId}/${page}/${pageSize}`;
    return this.httpClient.get<ArticlePagination>(api).pipe(
      tap(x => {
        this.loading = false;
      })
    )
  }
}
