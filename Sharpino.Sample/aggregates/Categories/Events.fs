
namespace Sharpino.Sample.Categories

open System
open Sharpino.Core
open Sharpino.Cache

open Sharpino.Sample.Models.CategoriesModel
open Sharpino.Sample.CategoriesAggregate

module CategoriesEvents =
    type CategoryEvent =
        | CategoryAdded of Category
        | CategoryRemoved of Guid
        | CategoriesAdded of List<Category>
            interface Event<CategoriesAggregate> with
                member this.Process (x: CategoriesAggregate) =
                    match this with
                    | CategoryAdded (c: Category) ->
                        EventCache<CategoriesAggregate>.Instance.Memoize (fun () -> x.AddCategory c) (x, [this])
                    | CategoryRemoved (g: Guid) ->
                        EventCache<CategoriesAggregate>.Instance.Memoize (fun () -> x.RemoveCategory g) (x, [this])
                    | CategoriesAdded (cs: List<Category>) ->
                        EventCache<CategoriesAggregate>.Instance.Memoize (fun () -> x.AddCategories cs) (x, [this])