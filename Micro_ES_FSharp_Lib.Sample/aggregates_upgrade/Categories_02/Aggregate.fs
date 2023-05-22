
namespace Tonyx.EventSourcing.Sample_02

open Tonyx.EventSourcing.Sample.Todos.Models.CategoriesModel
open System

module CategoriesAggregate =
    type CategoriesAggregate =
        {
            Categories: Categories
        }
        static member Zero =
            {
                Categories = Categories.Zero
            }
        // storagename _MUST_ be unique for each aggregate and the relative lock object 
        // must be added in syncobjects map in Conf.fs
        static member StorageName =
            "_categories"
        static member Version =
            "_02"
        member this.AddCategory(c: Category) =
            ceResult {
                let! result = this.Categories.AddCategory(c)
                let result =
                    {
                        this with
                            Categories = result
                    }
                return result
            }
        member this.RemoveCategory(id: Guid) =
            ceResult {
                let! result = this.Categories.RemoveCategory(id)
                let result =
                    {
                        this with
                            Categories = result
                    }
                return result
            }
        member this.GetCategories() = this.Categories.GetCategories()