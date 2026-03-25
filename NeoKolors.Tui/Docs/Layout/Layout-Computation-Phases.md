# Layout Computation Phases

There are three element layout computation phases in total:
1. Minimum width layout computation
2. Maximum width layout computation
3. Render layout computation

## Minimum width layout computation

The minimum width layout is a layout where the content box has the [minimum intrinsic
size](https://developer.mozilla.org/en-US/docs/Glossary/Intrinsic_Size#minimum_intrinsic_size).
For example, in the case of a text element, this would be when the width of the content box
is equal to the width of the longest word of the text. 

The minimum width layout is used when computing the layout of a parent element. The computation
should also take into account the styles of the element.

The example below shows a text rendered with minimum intrinsic size.

<p style="width: min-content; border: 1px solid white; padding: 5px;">
Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed
</p>

## Maximum width layout computation

The maximum width layout is a layout where the content box has the [maximum intrinsic
size](https://developer.mozilla.org/en-US/docs/Glossary/Intrinsic_Size#maximum_intrinsic_size).
For example, in the case of a text element, this would be when the width of the content box
is equal to the width of the longest line of the text.

The maximum width layout is used when computing the layout of a parent element. The computation
should also take into account the styles of the element.

The example below shows a text rendered with maximum intrinsic size.

<p style="width: max-content; border: 1px solid white; padding: 5px;">
Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed
</p>

## Render layout computation

The render layout is calculated before the rendering of the element. It is used to
determine the overall layout of the element before it is rendered.

<p style="width: 100px; border: 1px solid white; padding: 5px;">
Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed
</p>

## See Also

- [Intrinsic Size](https://developer.mozilla.org/en-US/docs/Glossary/Intrinsic_Size)