module Interpreters.Logo.Tests.Helper

open Repo

let repo = RepoFactory.Create()
let metamodelName = "LogoMetamodel"

module Metamodel =   
    let metamodel = repo.Model metamodelName
    let metamodelAbstractNode = metamodel.FindElement "AbstractNode"
     
    let link = metamodel.FindElement "Link"
    
    let taggedLink = metamodel.FindElement "TaggedLink"

    let metamodelInitialNode = metamodel.FindElement "InitialNode"
    let metamodelFinalNode = metamodel.FindElement "FinalNode"

    let metamodelForward = metamodel.FindElement "Forward"
    let metamodelBackward = metamodel.FindElement "Backward"
    let metamodelRight = metamodel.FindElement "Right"
    let metamodelLeft = metamodel.FindElement "Left"
    let metamodelPenUp = metamodel.FindElement "PenUp"
    let metamodelPenDown = metamodel.FindElement "PenDown"

    let metamodelRepeat = metamodel.FindElement "Repeat"
    
open Metamodel

let modelName = "LogoTest"

let private model = repo.CreateModel(modelName, metamodelName)

let getModel() = model

let resetModel() =
    let elements = model.Elements
    for i in elements do model.RemoveElement(i)
    
/// Creates an element of this type and adds to model.
let (+++) (``type``: IElement) =
    model.CreateElement ``type``
  
/// Creates a link and return target.     
let (-->) source target =
    let edge = (model.CreateElement link) :?> IEdge
    edge.From <- source
    edge.To <- target
    target

/// Creates a tagged link and return it.
let (+->) source target =
    let edge = (model.CreateElement taggedLink) :?> IEdge
    edge.From <- source
    edge.To <- target
    edge
    
let setAttribute (element: IElement) attr newValue =
    ElementHelper.setAttributeValue attr newValue element
    
let createForward distance = 
    let forward = (+++) metamodelForward 
    do setAttribute forward "Distance" distance
    forward

let createBackward distance =
    let backward = (+++) metamodelBackward
    do setAttribute backward "Distance" distance
    backward

let createRight degrees =
    let right = (+++) metamodelRight
    do setAttribute right "Degrees" degrees
    right

let createLeft degrees =
    let left = (+++) metamodelLeft 
    do setAttribute left "Degrees" degrees
    left

let createRepeat count =
    let repeat = (+++) metamodelRepeat
    do setAttribute repeat "Count" count
    repeat

let createPenUp() = (+++) metamodelPenUp

let createPenDown() = (+++) metamodelPenDown

let createInitialNode() = (+++) metamodelInitialNode

let createFinalNode() = (+++) metamodelFinalNode
