namespace Sharpino.Sample
open System

open Sharpino.Sample.Models.CategoriesModel
open Sharpino.Sample.Models.TodosModel
open Sharpino.Utils

open FSharpPlus
open FsToolkit.ErrorHandling

module TodosAggregate =
    type TodosAggregate =
        {
            todos: Todos
            categories: Categories
        }
        static member Zero =
            {
                todos = Todos.Zero
                categories = Categories.Zero
            }
        static member StorageName =
            "_todo"
        static member Version =
            "_01"
        static member SnapshotsInterval =
            15
        member this.AddTodo (t: Todo) =
            let checkCategoryExists (c: Guid ) =
                this.categories.GetCategories() 
                |> List.exists (fun x -> x.Id = c) 
                |> boolToResult (sprintf "A category with id '%A' does not exist" c)

            ResultCE.result
                {
                    let! categoriesMustExist = t.CategoryIds |> catchErrors checkCategoryExists
                    let! todos = this.todos.AddTodo t
                    return 
                        {
                            this with
                                todos = todos
                        }
                }
        member this.RemoveTodo (id: Guid) =
            ResultCE.result
                {
                    let! todos = this.todos.RemoveTodo id
                    return
                        {
                            this with
                                todos = todos
                        }
                }
        member this.GetTodos() = this.todos.GetTodos()
        member this.AddCategory (c: Category) =
            ResultCE.result
                {
                    let! categories = this.categories.AddCategory c
                    return
                        {
                            this with
                                categories = categories
                        }
                }
        member this.RemoveCategory (id: Guid) = 
            let removeReferenceOfCategoryToTodos (id: Guid) =
                this.todos.todos 
                |>>
                (fun x -> 
                    { x with 
                        CategoryIds = 
                            x.CategoryIds 
                            |> List.filter (fun y -> y <> id)}
                )
            
            ResultCE.result
                {
                    let! categories = this.categories.RemoveCategory id
                    return
                        {
                            this with
                                categories = categories
                                todos = 
                                    {
                                        this.todos 
                                            with todos = removeReferenceOfCategoryToTodos id
                                    }
                        }
                }   
        member this.RemoveTagReference (id: Guid) =
            let removeReferenceOfTagToAllTodos (id: Guid) =
                this.todos.todos 
                |>> 
                (fun x -> 
                    { x with 
                        TagIds = 
                            x.TagIds 
                            |> List.filter (fun y -> y <> id)}
                )
            ResultCE.result
                {
                    return
                        {
                            this with
                                todos = 
                                    {
                                        this.todos 
                                            with todos = removeReferenceOfTagToAllTodos id
                                    }
                        }
                }
        member this.GetCategories() = this.categories.GetCategories()

// what follows is the same code as above, but with the new version of the aggregate
    [<UpgradedVersion>]

    type TodosAggregate' =
        {
            todos: Todos
        }
        static member Zero =
            {
                todos = Todos.Zero
            }
        // storagename _MUST_ be unique for each aggregate and the relative lock object 
        // must be added in syncobjects map in Conf.fs
        static member StorageName =
            "_todo"
        static member Version =
            "_02"
        static member SnapshotsInterval =
            15
        member this.AddTodo (t: Todo) =
            ResultCE.result
                {
                    let! todos = this.todos.AddTodo t
                    return
                        {
                            this with
                                todos = todos
                        }
                }
        member this.RemoveTodo (id: Guid) =
            ResultCE.result
                {
                    let! todos = this.todos.RemoveTodo id
                    return
                        {
                            this with
                                todos = todos
                        }
                }

        member this.AddTodos (ts: List<Todo>) =
            ResultCE.result
                {
                    let! todos = this.todos.AddTodos ts
                    return
                        {
                            this with
                                todos = todos
                        }
                }

        member this.GetTodos() = this.todos.GetTodos()

        member this.RemoveCategoryReference (id: Guid) =
            let removeReferenceOfCategoryToTodos (id: Guid) =
                this.todos.todos 
                |>>
                (fun x -> 
                    { x with 
                        CategoryIds = 
                            x.CategoryIds 
                            |> List.filter (fun y -> y <> id)}
                )
            
            ResultCE.result
                {
                    return
                        {
                            this with
                                todos = 
                                    {
                                        this.todos 
                                            with todos = removeReferenceOfCategoryToTodos id
                                    }
                        }
                }

        member this.RemoveTagReference (id: Guid) =
            let removeReferenceOfTagToAllTodos (id: Guid) =
                this.todos.todos 
                |>> 
                (fun x -> 
                    { x with 
                        TagIds = 
                            x.TagIds 
                            |> List.filter (fun y -> y <> id)}
                )
            ResultCE.result
                {
                    return
                        {
                            this with
                                todos = 
                                    {
                                        this.todos 
                                            with todos = removeReferenceOfTagToAllTodos id
                                    }
                        }
                }