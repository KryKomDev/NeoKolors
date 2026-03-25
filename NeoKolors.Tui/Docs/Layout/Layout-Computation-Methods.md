# Layout Computation Methods

This page describes how the layout computation methods from `NeoKolors.Tui.Elements.IElement`
work and how you should use them.

## The Idea

There are methods that work in two opposite directions:
1. For computing the content box size from the parent bounds box
2. For computing the element size from an initial content box size

The methods apply styles for width and height manipulation and margin, border and padding 
styles to achieve an accurate element layout.

## Usage

The methods should be used accordingly to the case you want to solve.

### 1. Get the size of the content box

This is useful if you are computing the layout for the render phase of layout computation.


### 2. Get the size of the element with a certain content size

This is useful when you are trying to compute the min and max layout sizes 
in the min/max width layout computation phase.


## See Also

- [Layout Computation Phases](./Layout-Computation-Phases.md)