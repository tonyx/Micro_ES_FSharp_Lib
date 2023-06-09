
namespace Sharpino.Sample

open FsToolkit.ErrorHandling

open Sharpino.Sample.Models.CategoriesModel
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
        static member StorageName =
            "_categories"
        static member Version =
            "_02"
        static member SnapshotsInterval =
            15
        member this.AddCategory(c: Category) =
            ResultCE.result {
                let! categories = this.Categories.AddCategory(c)
                return
                    {
                        this with
                            Categories = categories
                    }
            }
        member this.RemoveCategory(id: Guid) =
            ResultCE.result {
                let! categories = this.Categories.RemoveCategory(id)
                return
                    {
                        this with
                            Categories = categories
                    }
            }
        member this.AddCategories(cs: List<Category>) =
            ResultCE.result {
                let! categories = this.Categories.AddCategories(cs)
                return
                    {
                        this with
                            Categories = categories
                    }
            }
        member this.GetCategories() = this.Categories.GetCategories()