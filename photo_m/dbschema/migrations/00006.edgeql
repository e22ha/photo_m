CREATE MIGRATION m1xruhlkjb52ip5jyv42iwkpcfx6ptifdlbd7oziyy5whbaxmxllaq
    ONTO m1zwx7urf6xcqwejspnvjcaq25cpzqwbutf3xo3ekfva22bumufyyq
{
  CREATE FUNCTION default::count_p_by_author(author: default::Photographer) ->  std::int64 USING (SELECT
      std::count((SELECT
          default::Photo
      FILTER
          (.author = author)
      ))
  );
};
