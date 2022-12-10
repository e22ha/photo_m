CREATE MIGRATION m1zwx7urf6xcqwejspnvjcaq25cpzqwbutf3xo3ekfva22bumufyyq
    ONTO m1himwh7hutowj2y5frzcdjrazbn7wwgvh25yskvc3kcchvq7ixhsq
{
  ALTER TYPE default::Photographer {
      CREATE PROPERTY nick -> std::str {
          CREATE CONSTRAINT std::exclusive;
      };
  };
};
