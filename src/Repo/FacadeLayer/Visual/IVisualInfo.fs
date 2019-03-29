namespace Repo.Visual

/// This interface represents information about how element is shown on screen.
type IVisualInfo =
    interface
        /// Address to xml file. 
        abstract XmlDescription : string with get, set 
    end

/// This interface represents information about how node is shown on screen.
type IVisualNodeInfo =
    interface
        inherit IVisualInfo

        /// Position of node on screen
        abstract Position : (int * int) option with get, set          
    end

// This interface represents information about how edge is shown on screen.
type IVisualEdgeInfo =
    interface
        inherit IVisualInfo

        /// Coordinates of routing points without ends
        abstract RoutingPoints : (int * int) list with get, set
    end